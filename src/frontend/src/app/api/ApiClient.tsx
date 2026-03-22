import { toastError } from "../../services/ToastService";

//file xu ly goi api dung chung, dung env cho base url, xu ly error voi toast
interface ApiResponse<T> {
    code: number;
    message: string;
    data: { accessToken: string } & T;
    //data?: T;
}

class ApiClient {
    private baseUrl: string;
    private token: string | null = null; //luu token trong apiclient
    private isRefreshing = false;
    private refreshPromise: Promise<string | null> | null = null;
    constructor(endpoint: string) {
        this.baseUrl = `${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api/v1'}${endpoint}`;
    }

    //ham setToken de authContext day token vao day
    public setToken(newToken: string | null) {
        this.token = newToken;
    }

    //ham lay ra session id
    private getOrCreateSessionId(): string {
        let sessionId = sessionStorage.getItem('sessionId'); //dung sessionStorage thay vi local de session-based
        if (!sessionId) {
            sessionId = `sess_${crypto.randomUUID()}`;
            sessionStorage.setItem('sessionId', sessionId);
        }
        return sessionId;
    }

    //dung url voi params
    private buildApiUrl(endpoint: string, additionalParams: Record<string, any> = {}): string {
        const params = new URLSearchParams(additionalParams);
        return `${this.baseUrl}${endpoint}?${params.toString()}`;
    }

    private async refreshToken(): Promise<string | null> {
        try {
            const response = await fetch(`${this.baseUrl}/auth/refresh-token`, {
                method: 'POST',
                credentials: 'include',
            });

            if (!response.ok) throw new Error('Failed to refresh token');

            const data = await response.json();
            return data.accessToken;
        } catch (error) {
            console.error('Failed to refresh token:', error);
            return null;
        }
    }

    //wrapper cho fetch, xu ly header va error
    private async apiCall<T>(url: string, options: RequestInit = {}, retry = true): Promise<ApiResponse<T>> {
        try {
            const response = await fetch(url, {
                ...options,
                            //them credentials de su dung httpOnly cookie
                credentials: 'include', //bat buoc de nhan httpOnly cookie xu ly tu .NET
                headers: {
                    'Content-Type': 'application/json',
                    'X-Session-Id': this.getOrCreateSessionId(),
                    //neu co token thi gan Authorization header
                    ...(this.token ? { 'Authorization': `Bearer ${this.token}` } : {}),
                    ...options.headers,
                },
            });

            //auto refresh token neu 401 va retry = true
            if (response.status === 401 && retry) {
                if (!this.isRefreshing) {
                    this.isRefreshing = true;
                    this.refreshPromise = this.refreshToken();
                }

                const newToken = await this.refreshPromise;
                this.isRefreshing = false;

                if (!newToken) {
                    throw new Error('UNAUTHORIZED');
                }

                this.token = newToken;

                //thu lai api call voi token moi
                return this.apiCall<T>(url, options, false);
            }

            const data: ApiResponse<T> = await response.json();

            if (!response.ok || data.code >= 400) {
                throw new Error(data.message || 'API call failed');
            }

            return data;
        } catch (error: any) {
            toastError(error.message || "Unknown API error");
            throw error;
        }
    }

    //cac phuong thuc public de sd o service con
    public async get<T>(endpoint: string, params: Record<string, any> = {}): Promise<ApiResponse<T>> {
        return this.apiCall<T>(this.buildApiUrl(endpoint, params), { method: 'GET' }); 
    }

    public async post<T>(endpoint: string, body: any, params: Record<string, any> = {}): Promise<ApiResponse<T>> {
        return this.apiCall(this.buildApiUrl(endpoint, params), {
            method: 'POST',
            body: JSON.stringify(body)
        });
    }

    public async put<T>(endpoint: string, body: any, params: Record<string, any> = {}): Promise<ApiResponse<T>> {
        return this.apiCall<T>(this.buildApiUrl(endpoint, params), {
            method: 'PUT',
            body: JSON.stringify(body)
        });
    }

    public async delete<T>(endpoint: string, params: Record<string, any> = {}): Promise<ApiResponse<T>> {
        return this.apiCall<T>(this.buildApiUrl(endpoint, params), { method: 'DELETE' });
    }
}

//tao 1 instance dung chung cho toan bo app (singleton
export const globalApiClient = new ApiClient('');
export default ApiClient;
