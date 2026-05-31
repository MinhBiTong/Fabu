export type ApiResponse<T> = {
  code: number;
  message: string;
  data: T;
};

export type ClaimDto = {
  type: string;
  value: string;
};

export type LoginResponse = {
  accessToken: string;
  refreshToken?: string;
  expiresAt?: string;
  claims: ClaimDto[];
};

export type RegisterResponse = {
  userId: number;
  email: string;
  phoneNumber: string;
  message: string;
  requiresOtpVerification: boolean;
};

export type RegisterRequest = {
  Username: string;
  FullName: string;
  PhoneNumber: string;
  Email: string;
  Password: string;
};

export type ServicePlan = {
  id: number;
  serviceName: string;
  serviceCode: string;
  category: string;
  dataAmountMB: number;
  validityDays: number;
  price: number;
  description: string;
  isAutoRenew: boolean;
  maxActivationsPerMonth: number;
  isActive: boolean;
  createdAt?: string;
};

export type ServicePlanPayload = Omit<ServicePlan, "id" | "createdAt">;

export type RechargePlan = {
  id: number;
  name: string;
  price: number;
  points: number;
  description?: string | null;
};

export type FeedbackRequest = {
  customerId: number | null;
  subject: string;
  message: string;
  rating: number;
};

export type FeedbackResponse = {
  id: number;
  email?: string;
  subject?: string;
  message?: string;
  content?: string;
  rating: number;
  status?: number | string;
  customerId?: number | null;
  createdAt?: string;
};

export type TransactionResponse = {
  customerId?: number | null;
  paymentId?: number | null;
  transactionType: string;
  amount: number;
  status: string;
  paymentMethod: string;
  transactionRef: string;
  completedAt?: string | null;
  couponUsageCount?: number;
};

export type TransactionCreateRequest = {
  CustomerId: number | null;
  PaymentId: number | null;
  TransactionType: "Recharge" | "BillPayment" | "Service";
  Amount: number;
  Status: 0 | 1 | 2;
  PaymentMethod: 0 | 1 | 2 | 3;
  TransactionRef: string;
  CompletedAt: string;
  CouponCode?: string | null;
  MobileNumber?: string | null;
};

export type UserSummary = {
  id?: number;
  username?: string;
  userName?: string;
  fullName?: string;
  email: string;
  role?: string;
  phoneNumber?: string;
  createdDate?: string;
  dateOfBirth?: string;
};

export type ChatbotMessageRequest = {
  customerId?: number | null;
  sessionId?: string | null;
  message: string;
  resetContext?: boolean;
};

export type ChatbotMessageResponse = {
  sessionId: string;
  answer: string;
  provider: string;
  model: string;
  isFallback: boolean;
  generatedAt: string;
  retrievedSources: string[];
  suggestedActions: string[];
};
