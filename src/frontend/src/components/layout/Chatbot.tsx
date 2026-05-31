"use client";

import { FormEvent, memo, useCallback, useState } from "react";
import { Button } from "@/components/ui/button";
import { chatbotService } from "@/services/chatbot-service";

type ChatMessage = {
  id: string;
  role: "user" | "assistant";
  content: string;
};

function Chatbot() {
  const [isOpen, setIsOpen] = useState(false);
  const [sessionId, setSessionId] = useState<string | null>(null);
  const [message, setMessage] = useState("");
  const [messages, setMessages] = useState<ChatMessage[]>([
    {
      id: "welcome",
      role: "assistant",
      content: "Welcome to Fabu support. Ask about recharge, data plans, or account services.",
    },
  ]);
  const [isSending, setSending] = useState(false);

  const handleSubmit = useCallback(
    async (event: FormEvent) => {
      event.preventDefault();
      const content = message.trim();
      if (!content || isSending) return;

      setMessage("");
      setSending(true);
      setMessages((current) => [
        ...current,
        { id: `user-${Date.now()}`, role: "user", content },
      ]);

      try {
        const response = await chatbotService.sendMessage({
          sessionId,
          message: content,
        });
        setSessionId(response.sessionId);
        setMessages((current) => [
          ...current,
          {
            id: `assistant-${Date.now()}`,
            role: "assistant",
            content: response.answer,
          },
        ]);
      } catch {
        setMessages((current) => [
          ...current,
          {
            id: `assistant-error-${Date.now()}`,
            role: "assistant",
            content: "Support is temporarily unavailable. Please try again shortly.",
          },
        ]);
      } finally {
        setSending(false);
      }
    },
    [isSending, message, sessionId]
  );

  if (!isOpen) {
    return (
      <button
        type="button"
        className="fixed bottom-5 right-5 z-30 flex h-14 w-14 items-center justify-center rounded-full bg-fabu-red text-lg font-bold text-white shadow-modal hover:bg-fabu-red-hover"
        onClick={() => setIsOpen(true)}
        aria-label="Open Fabu support"
      >
        ?
      </button>
    );
  }

  return (
    <section className="fixed bottom-5 right-5 z-30 flex h-[560px] w-[min(420px,calc(100vw-32px))] flex-col rounded-card border border-fabu-border bg-white shadow-modal">
      <div className="flex min-h-16 items-center justify-between border-b border-fabu-border px-4">
        <div>
          <h3 className="text-lg">Fabu Support</h3>
          <p className="text-xs text-fabu-gray">AI assistant</p>
        </div>
        <button
          type="button"
          className="flex h-11 w-11 items-center justify-center rounded-full bg-fabu-muted hover:bg-[#E7E7E7]"
          onClick={() => setIsOpen(false)}
          aria-label="Close support"
        >
          x
        </button>
      </div>

      <div className="flex-1 space-y-3 overflow-y-auto p-4">
        {messages.map((item) => (
          <div
            key={item.id}
            className={`max-w-[82%] rounded-card p-3 text-sm leading-6 ${
              item.role === "user"
                ? "ml-auto bg-fabu-red text-white"
                : "bg-fabu-muted text-fabu-charcoal"
            }`}
          >
            {item.content}
          </div>
        ))}
      </div>

      <form className="flex gap-2 border-t border-fabu-border p-3" onSubmit={handleSubmit}>
        <input
          className="fabu-input"
          value={message}
          onChange={(event) => setMessage(event.target.value)}
          placeholder="Type a message"
        />
        <Button type="submit" disabled={isSending}>
          Send
        </Button>
      </form>
    </section>
  );
}

export default memo(Chatbot);
