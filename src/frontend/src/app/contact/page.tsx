import { ContactForm } from "@/components/ui/Form/contact-form";

export default function ContactPage() {
  return (
    <div className="ContactContainer">
          <h1 className="text-3xl font-bold tracking-tighter sm:text-4xl">Contact Us</h1>
       
      <div className="ContactContent">
      
             <h3>How would you rate your experience?</h3>
           <div className="Starsrating"> 
            <span>⭐</span>
            <span>⭐</span>
            <span>⭐</span>
            <span>⭐</span>
            <span>⭐</span> 
           </div>
           <h3>Any suggestions for improvement? Send us a message!</h3>
 
             <div className="ContactForm">
              
              <p>Email</p>
               <input type="text" placeholder="Enter your email" />
              <p>Subject</p>
               <input type="text" placeholder="Enter subject" />
              <p>Message</p>
               <textarea placeholder="Enter your message" />

                <button type="submit">Submit</button>
                </div>
                </div>
            </div>
  );
}