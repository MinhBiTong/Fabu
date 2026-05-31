import type {
  RechargePlan,
  TransactionCreateRequest,
  TransactionResponse,
} from "@/core/types/api.types";
import { endpoints } from "@/lib/api/endpoints";
import { globalApiClient } from "@/lib/api/http-client";

export type RechargePayload = {
  phone: string;
  amount: number;
  coupon?: string;
  paymentMethod?: 0 | 1 | 2 | 3;
};

export const rechargeService = {
  async listPlans() {
    const response = await globalApiClient.get<RechargePlan[]>(endpoints.rechargePlans.active);
    return response.data ?? [];
  },

  async createRecharge(payload: RechargePayload) {
    const body: TransactionCreateRequest = {
      CustomerId: null,
      PaymentId: null,
      TransactionType: "Recharge",
      Amount: payload.amount,
      Status: 0,
      PaymentMethod: payload.paymentMethod ?? 2,
      TransactionRef: `RCH-${Date.now()}`,
      CompletedAt: new Date().toISOString(),
      CouponCode: payload.coupon || null,
      MobileNumber: payload.phone,
    };

    const response = await globalApiClient.post<TransactionResponse>(
      endpoints.transactions.recharge,
      body
    );
    return response.data;
  },
};

export const getPackages = rechargeService.listPlans;
export const rechargeApi = rechargeService.createRecharge;
