"use client";

import Image from "next/image";
import Star from "../../../../styles/images/StarratingYes.png";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import { globalApiClient } from "@/app/api/ApiClient";

type Feedback = {
  id: number;
  subject: string;
  message: string;
  rating: number;
  status: number;
  customerName?: string;
  email?: string;
};

export default function FeedbackDetails() {
 type Feedback = {
    id: number;
    email: string;
    rating: number;
    subject: string;
    message: string;
    status: number; // 0 = unread, 1 = read
  };

  const [feedback, setFeedback] = useState<Feedback | null>(null);

  const params = useParams();
  const id = Array.isArray(params.id) ? params.id[0] : params.id;

  // ✅ FETCH ONLY
  useEffect(() => {
    const fetchFeedback = async () => {
      try {
        const token = localStorage.getItem("accessToken");
        globalApiClient.setToken(token);

        const res = await globalApiClient.get<Feedback>(`Feedbacks/${id}`);

     
        setFeedback(res.data);

      } catch (err) {
        console.error("DETAIL ERROR:", err);
      }
    };

    if (id) fetchFeedback();
  }, [id]);

  if (!feedback) return <div>Loading...</div>;


  return (
    <div className="AdminFeedDetailsContainer">

      <h1>{feedback.customerName || "Anonymous"}'s Feedback</h1>

      {/* ⭐ Stars */}
      <div className="StarsRated">
        {Array.from({ length: feedback.rating }).map((_, i) => (
          <Image key={i} src={Star} alt="star" />
        ))}
      </div>

      {/* 👤 Info */}
      <div className="Nameplace">
        <span>UserName: {feedback.customerName || "Unknown"}</span>
        <span>Email: {feedback.email || "N/A"}</span>
      </div>

      {/* 📝 Content */}
      <div className="DisplayContent">
        <h2>Subject: {feedback.subject}</h2>
        <span>{feedback.message}</span>
      </div>

    </div>
  );
}