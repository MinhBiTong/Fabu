import type {
  CartCheckoutRequest,
  CartItemRequest,
  CartResponse,
  OrderResponse,
  PackagePaymentRequest,
  TelecomProduct,
} from "@/core/types/api.types";
import { endpoints } from "@/lib/api/endpoints";
import { globalApiClient } from "@/lib/api/http-client";

export type ProductQuery = {
  keyword?: string;
  category?: string;
  includeInactive?: boolean;
};

export type CheckoutResult = {
  order: OrderResponse;
  paymentUrl?: string | null;
  paymentRef?: string | null;
};

export const commerceService = {
  getProducts(params?: ProductQuery) {
    return globalApiClient.get<TelecomProduct[]>(endpoints.commerce.products, params);
  },

  getFeaturedProducts() {
    return globalApiClient.get<TelecomProduct[]>(endpoints.commerce.featuredProducts);
  },

  getProduct(productId: string) {
    return globalApiClient.get<TelecomProduct>(endpoints.commerce.productDetail(productId));
  },

  getCart(customerId: number) {
    return globalApiClient.get<CartResponse>(endpoints.commerce.cartByCustomer(customerId));
  },

  addCartItem(payload: CartItemRequest) {
    return globalApiClient.post<CartResponse>(endpoints.commerce.cartItem, payload);
  },

  updateCartItem(payload: CartItemRequest) {
    return globalApiClient.put<CartResponse>(endpoints.commerce.cartItem, payload);
  },

  removeCartItem(customerId: number, productId: string) {
    return globalApiClient.delete<CartResponse>(
      endpoints.commerce.cartItemForProduct(customerId, productId)
    );
  },

  checkout(payload: CartCheckoutRequest) {
    return globalApiClient.post<CheckoutResult>(endpoints.commerce.checkout, payload);
  },

  getOrder(orderId: string) {
    return globalApiClient.get<OrderResponse>(endpoints.commerce.orderDetail(orderId));
  },

  getOrdersByCustomer(customerId: number) {
    return globalApiClient.get<OrderResponse[]>(endpoints.commerce.ordersByCustomer(customerId));
  },

  createPackagePayment(payload: PackagePaymentRequest) {
    return globalApiClient.post<unknown>(endpoints.commerce.packagePayment, payload);
  },
};
