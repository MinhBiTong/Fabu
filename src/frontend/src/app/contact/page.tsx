"use client"

import { ContactForm } from "@/components/ui/Form/contact-form";
import Image from "next/image";
import { useState } from "react";
import { globalApiClient } from "@/app/api/api-client";
import { feedbackSchema } from "@/core/validations/feedback.schema";
import Phone from "../../styles/images/phonecall.png";
import Starrate from "../../styles/images/Starating.png"
import Starrateyes from "../../styles/images/StarratingYes.png"
import Location2 from "../../styles/images/location2.png";
import Gmail from "../../styles/images/gmail.png"

export default function ContactPage() {
  const [hovered, setHovered] = useState(0);
  const [rating, setRating] = useState(0);

  // ✅ NEW STATE
  const [form, setForm] = useState({

    subject: "",
    message: ""
  });

  const [errors, setErrors] = useState<any>({});

  // ✅ HANDLE INPUT
  const handleChange = (e: any) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

const handleSubmit = async () => {
  const result = feedbackSchema.safeParse({
 
    subject: form.subject,
    message: form.message,
    rating: rating
  });

  if (!result.success) {
    const fieldErrors: any = {};

    result.error.issues.forEach((err) => {
      fieldErrors[err.path[0]] = err.message;
    });

    setErrors(fieldErrors);
    return;
  }

  try {
    const token = localStorage.getItem("accessToken");
    if (token) globalApiClient.setToken(token);

    const res = await globalApiClient.post<any>("Feedbacks", {
  
      subject: form.subject,
      message: form.message,
      rating: rating,
      status: 0,
      customerId: null
    });

    if (res.code === 200) {
      alert("Feedback submitted successfully!");

      setForm({
     
        subject: "",
        message: ""
      });
      setRating(0);
      setErrors({});
    } else {
      alert("Failed to submit feedback");
    }

  } catch (err) {
    console.error(err);
    alert("Server error");
  }
};
  return (
    <div className="ContactContainer">
      <h1>Contact Us</h1>

      <div className="ContactContent">

        <h3>How would you rate your experience?</h3>

        <div className="Starsrating">
          {[1, 2, 3, 4, 5].map((star) => (
            <Image
              key={star}
              src={star <= (hovered || rating) ? Starrateyes : Starrate}
              alt=""
              onClick={() => setRating(star)}
              onMouseEnter={() => setHovered(star)}
              onMouseLeave={() => setHovered(0)}
              style={{ cursor: "pointer" }}
            />
          ))}
        </div>

        {/* ✅ rating error */}
        {errors.rating && <span className="error">{errors.rating}</span>}

        <h3>Any suggestions for improvement? Send us a message!</h3>

        <div className="ContactForm">

          <p>Subject</p>
          <input
            name="subject"
            value={form.subject}
            onChange={handleChange}
            placeholder="Enter subject"
          />
          {errors.subject && <span className="error">{errors.subject}</span>}

          <p>Message</p>
          <textarea
            name="message"
            value={form.message}
            onChange={handleChange}
            placeholder="Enter your message"
          />
          {errors.message && <span className="error">{errors.message}</span>}

          <button type="button" onClick={handleSubmit}>
            Submit
          </button>
        </div>
      </div>

      <h1>You want to contact us directly ?</h1>

      <div className="ContactInfos">
        <div className="ContactLine">
          <div className="ContactBox">
            <Image src={Phone} alt=""></Image>
            <h3>Phone Numbers</h3>
            <div className="Lines">
              <p>0924010294</p>
              <p>0694206767</p>
              <p>0694206767</p>
            </div>
          </div>

          <div className="ContactBox">
            <Image src={Location2} alt=""></Image>
            <h3>Phone Numbers</h3>
            <div className="Lines">
              <p>fjsdkahksdj ashjkdg d ashkjdh kjasd d jkasdjk dh...</p>
            </div>
          </div>
        </div>

        <div className="ContactLine">
          <div className="ContactBox">
            <Image src={Gmail} alt=""></Image>
            <h3>Phone Numbers</h3>
            <div className="Lines">
              <p>fjsdkahksdj ashjkdg d ashkjdh kjasd d jkasdjk dh...</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}