// Regex đơn giản
export const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export const isValidEmail = (email: string) => emailRegex.test(email);

// Debounce helper để chống spam click
export function debounce<T extends (...args: any[]) => void>(func: T, delay: number) {
  let timer: NodeJS.Timeout;
  return (...args: Parameters<T>) => {
    clearTimeout(timer);
    timer = setTimeout(() => func(...args), delay);
  };
}

export const validatePassword = (password: string): string | null => {
  if (password.length < 8) {
    return "Mật khẩu phải có ít nhất 8 ký tự.";
  }
  return null;
};

export const validateEmail = (email: string): string | null => {
  if (!emailRegex.test(email)) {
    return "Email khó hợp lệ.";
  }
  return null;
};

export const validateConfirmPassword = (password: string, confirmPassword: string): string | null => {
  if (password !== confirmPassword) {
    return "Mật khẩu xác nhận không khớp.";
  }
  return null;
};

export const validateUsername = (username: string): string | null => {
  if (username.length < 3) {
    return "Tên người dùng phải có ít nhất 3 ký tự.";
  }
  return null;
};

export const sortBy = <T>(array: T[], key: keyof T, ascending: boolean = true): T[] => {
  return array.sort((a, b) => {
    if (a[key] < b[key]) return ascending ? -1 : 1;
    if (a[key] > b[key]) return ascending ? 1 : -1;
    return 0;
  });
};

export const sortByPriceOfService = <T extends { price: number }>(array: T[], ascending: boolean = true): T[] => {
  return array.sort((a, b) => {
    if (a.price < b.price) return ascending ? -1 : 1;
    if (a.price > b.price) return ascending ? 1 : -1;
    return 0;
  });
};