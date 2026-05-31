import { RegisterForm } from "@/features/auth/RegisterForm";

export default function RegisterPage() {
  return (
    <section className="fabu-section">
      <div className="fabu-container flex min-h-[calc(100vh-220px)] items-center justify-center">
        <RegisterForm />
      </div>
    </section>
  );
}
