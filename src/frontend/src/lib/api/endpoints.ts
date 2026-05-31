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
  chatbot: {
    chat: "AIChatbot/chat",
  },
} as const;
