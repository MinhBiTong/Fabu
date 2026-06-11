export const endpoints = {
  auth: {
    login: "v1/Auth/login",
    register: "v1/Auth/register",
    refresh: "v1/Auth/refresh-token",
    logout: "v1/Auth/logout",
    google: "v1/Auth/signin-google",
    github: "v1/Auth/signin-github",
  },
  users: {
    me: "v1/Users/me",
    list: "v1/Users",
  },
  services: {
    list: "Service",
    detail: (id: number | string) => `Service/${id}`,
    create: "Service",
    update: (id: number | string) => `Service/${id}`,
    delete: (id: number | string) => `Service/${id}`,
    search: "Service/search",
  },
  rechargePlans: {
    list: "RechargePlans",
    active: "RechargePlans/active",
    popular: (top: number) => `RechargePlans/popular/${top}`,
  },
  feedbacks: {
    list: "Feedbacks",
    detail: (id: number | string) => `Feedbacks/${id}`,
    create: "Feedbacks",
    markRead: (id: number | string) => `Feedbacks/${id}/mark-read`,
  },
  transactions: {
    recharge: "v1/Transaction/recharge",
    byRef: (transactionRef: string) => `v1/Transaction/${transactionRef}`,
    byCustomer: (customerId: number | string) => `v1/Transaction/customer/${customerId}`,
  },
  commerce: {
    products: "v1/Products",
    featuredProducts: "v1/Products/featured",
    productDetail: (productId: number | string) => `v1/Products/${productId}`,
    cartByCustomer: (customerId: number | string) => `v1/Cart/customer/${customerId}`,
    cartItem: "v1/Cart/items",
    cartItemForProduct: (customerId: number | string, productId: number | string) =>
      `v1/Cart/customer/${customerId}/items/${productId}`,
    checkout: "v1/Cart/checkout",
    orderDetail: (orderId: number | string) => `v1/Orders/${orderId}`,
    orderByCode: (orderCode: string) => `v1/Orders/code/${orderCode}`,
    ordersByCustomer: (customerId: number | string) => `v1/Orders/customer/${customerId}`,
    packagePayment: "v1/Payment/package",
    postpaidPayment: (billId: number | string) => `v1/Postpaid/bills/${billId}/pay`,
  },
  chatbot: {
    chat: "AIChatbot/chat",
  },
} as const;
