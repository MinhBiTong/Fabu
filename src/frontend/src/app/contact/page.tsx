import { ContactForm } from "@/components/ui/Form/contact-form";

export default function ContactPage() {
  return (
    <div className="container mx-auto py-10 px-4">
      <div className="max-w-2xl mx-auto space-y-6">
        <div className="text-center">
          <h1 className="text-3xl font-bold tracking-tighter sm:text-4xl">Liên hệ với chúng tôi</h1>
          <p className="text-muted-foreground mt-2">
            Hệ thống nạp tiền Mobile Recharge luôn sẵn sàng hỗ trợ bạn 24/7.
          </p>
        </div>
        
        {/* Render Form liên hệ */}
        <ContactForm />
      </div>
    </div>
  );
}