// đóng vai trò là các HttpClient đơn thuần, chỉ gọi API và trả về dữ liệu. Logic xử lý dữ liệu phức tạp nên đẩy về Custom Hooks.
import { globalApiClient } from "../app/api/ApiClient";

export const getPackages = async () => {
  return await globalApiClient.get<any>("recharge/packages");
};

export const rechargeApi = async (payload: {
  phone: string;
  amount: number;
  coupon: string;
}) => {
  return await globalApiClient.post<any>("recharge", payload);
};