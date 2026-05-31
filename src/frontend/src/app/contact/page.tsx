import { ContactFeedbackForm } from "@/features/feedback/ContactFeedbackForm";

const contactCards = [
  { title: "Phone", body: "0924010294\n0694206767" },
  { title: "Email", body: "support@fabu.vn" },
  { title: "Office", body: "Fabu customer support center, Ho Chi Minh City" },
];

export default function ContactPage() {
  return (
    <section className="fabu-section">
      <div className="fabu-container grid gap-8 lg:grid-cols-[1fr_0.85fr]">
        <div>
          <h1>Contact Us</h1>
          <p className="mt-3 max-w-2xl text-sm leading-6 text-fabu-gray">
            Rate your experience or contact Fabu support directly.
          </p>
          <div className="mt-8 grid gap-4 sm:grid-cols-3 lg:grid-cols-1">
            {contactCards.map((card) => (
              <article key={card.title} className="fabu-card">
                <h3 className="text-xl">{card.title}</h3>
                <p className="mt-2 whitespace-pre-line text-sm leading-6 text-fabu-gray">
                  {card.body}
                </p>
              </article>
            ))}
          </div>
        </div>

        <ContactFeedbackForm />
      </div>
    </section>
  );
}
