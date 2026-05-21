export const formatDate = (date: Date | string | number): string => {
  return new Intl.DateTimeFormat("vi-VN", {
    day: "2-digit", month: "2-digit", year: "numeric"
  }).format(new Date(date));
};

export const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency", currency: "VND"
  }).format(amount);
};