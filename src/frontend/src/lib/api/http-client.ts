import type { ApiResponse, LoginResponse } from "@/core/types/api.types";

type RequestBody = BodyInit | Record<string, unknown> | null | undefined;

export class ApiError extends Error {
  status: number;
  payload: unknown;

  constructor(message: string, status: number, payload: unknown) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.payload = payload;
  }
}

const AUTH_REFRESH_PATH = "v1/Auth/refresh-token";

function normalizeApiBaseUrl() {
  const configured =
    process.env.NEXT_PUBLIC_API_BASE_URL ||
    process.env.NEXT_PUBLIC_API_URL ||
    "http://localhost:5000/api";

  const base = configured.replace(/\/+$/, "");
  return /\/api(\/v\d+)?$/i.test(base) ? base : `${base}/api`;
}

function hasBody(body: RequestBody) {
  return body !== undefined && body !== null;
}

function toBody(body: RequestBody) {
  if (!hasBody(body)) return undefined;
  if (body instanceof FormData || body instanceof Blob || typeof body === "string") {
    return body;
  }

  return JSON.stringify(body);
}

function normalizeResponse<T>(
  raw: Partial<ApiResponse<T>> & { Message?: string; Data?: T; Code?: number },
  status: number,
  statusText: string
): ApiResponse<T> {
  return {
    code: raw.code ?? raw.Code ?? status,
    message: raw.message ?? raw.Message ?? statusText,
    data: (raw.data ?? raw.Data ?? raw) as T,
  };
}

export class ApiClient {
  private readonly baseUrl: string;
  private accessToken: string | null = null;
  private refreshPromise: Promise<string | null> | null = null;

  constructor(baseUrl = normalizeApiBaseUrl()) {
    this.baseUrl = baseUrl;
  }

  setToken(token: string | null) {
    this.accessToken = token;
  }

  getToken() {
    return this.accessToken;
  }

  private generateSessionId() {
    if (typeof crypto !== "undefined" && "randomUUID" in crypto) {
      return `sess_${crypto.randomUUID()}`;
    }

    return `sess_${Math.random().toString(36).slice(2)}_${Date.now()}`;
  }

  private getSessionId() {
    if (typeof window === "undefined") return "server";

    const key = "fabu_session_id";
    const existing = window.sessionStorage.getItem(key);
    if (existing) return existing;

    const created = this.generateSessionId();
    window.sessionStorage.setItem(key, created);
    return created;
  }

  private buildUrl(path: string, params?: Record<string, unknown>) {
    if (/^https?:\/\//i.test(path)) return path;

    let cleanPath = path.replace(/^\/+/, "");
    if (/\/api\/v1$/i.test(this.baseUrl) && cleanPath.toLowerCase().startsWith("v1/")) {
      cleanPath = cleanPath.slice(3);
    }

    const url = new URL(`${this.baseUrl}/${cleanPath}`);
    Object.entries(params ?? {}).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== "") {
        url.searchParams.set(key, String(value));
      }
    });

    return url.toString();
  }

  private async parseResponse<T>(response: Response) {
    const text = await response.text();
    const raw = text ? JSON.parse(text) : {};
    return normalizeResponse<T>(raw, response.status, response.statusText);
  }

  private async refreshAccessToken() {
    const response = await fetch(this.buildUrl(AUTH_REFRESH_PATH), {
      method: "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
        "X-Session-Id": this.getSessionId(),
      },
    });

    if (!response.ok) return null;

    const result = await this.parseResponse<LoginResponse>(response);
    const token = result.data?.accessToken ?? null;
    this.setToken(token);
    return token;
  }

  private async request<T>(
    path: string,
    options: RequestInit = {},
    params?: Record<string, unknown>,
    retry = true
  ): Promise<ApiResponse<T>> {
    const headers = new Headers(options.headers);
    const body = options.body as RequestBody;

    if (hasBody(body) && !(body instanceof FormData) && !headers.has("Content-Type")) {
      headers.set("Content-Type", "application/json");
    }

    headers.set("X-Session-Id", this.getSessionId());
    if (this.accessToken) {
      headers.set("Authorization", `Bearer ${this.accessToken}`);
    }

    const response = await fetch(this.buildUrl(path, params), {
      ...options,
      body: toBody(body),
      credentials: "include",
      headers,
    });

    if (response.status === 401 && retry && path !== AUTH_REFRESH_PATH) {
      this.refreshPromise ??= this.refreshAccessToken().finally(() => {
        this.refreshPromise = null;
      });

      const newToken = await this.refreshPromise;
      if (newToken) {
        return this.request<T>(path, options, params, false);
      }
    }

    const result = await this.parseResponse<T>(response);
    if (!response.ok) {
      throw new ApiError(result.message || "Request failed", response.status, result);
    }

    return result;
  }

  get<T>(path: string, params?: Record<string, unknown>) {
    return this.request<T>(path, { method: "GET" }, params);
  }

  post<T>(path: string, body?: RequestBody, params?: Record<string, unknown>) {
    return this.request<T>(path, { method: "POST", body: body as BodyInit }, params);
  }

  put<T>(path: string, body?: RequestBody, params?: Record<string, unknown>) {
    return this.request<T>(path, { method: "PUT", body: body as BodyInit }, params);
  }

  delete<T>(path: string, params?: Record<string, unknown>) {
    return this.request<T>(path, { method: "DELETE" }, params);
  }
}

export const globalApiClient = new ApiClient();
