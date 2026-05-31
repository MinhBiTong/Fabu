import { LoginForm } from "@/features/auth/LoginForm";

export default function LoginPage() {
  return (
    <section className="fabu-section">
      <div className="fabu-container flex min-h-[calc(100vh-220px)] items-center justify-center">
        <LoginForm />
      </div>
    </section>
  );
}
