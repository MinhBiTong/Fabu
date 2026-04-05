"use client"

import {    BarChart,Bar, XAxis, YAxis,Tooltip, ResponsiveContainer} from "recharts";
import Image from "next/image";

import { useEffect, useState } from "react";
import { globalApiClient } from "@/app/api/ApiClient";

import { useRouter } from "next/navigation";
import Way from "../../styles/images/Way.png"
import Star from "../../styles/images/StarratingYes.png"
import search from "../../styles/images/search.png"

export default function AdminFeedbacks() {

const router = useRouter()


const [feedbacks, setFeedbacks] = useState<Feedback[]>([]);

const data = [
  { star: "5", total: 30 },
  { star: "4", total: 10 },
  { star: "3", total: 5 },
  { star: "2", total: 12 },
  { star: "1", total: 5 },
];

type Feedback = {
  id: number;
  subject: string;
  rating: number;
};

useEffect(() => {
  const fetchFeedbacks = async () => {
    try {
         const token = localStorage.getItem("accessToken");
        globalApiClient.setToken(token);
        
      const res = await globalApiClient.get("/Feedbacks");

      console.log("DATA:", res.data);

      setFeedbacks(Array.isArray(res.data) ? res.data : []);

    } catch (err) {
      console.error(err);
    }
  };

  fetchFeedbacks();
}, []);

  
  return ( 
  <>
  <div className="AdminFeedbacksContainer"> 
   <div className="TotalStarContainer">
    <h2>Stars Chart</h2>
    <ResponsiveContainer>
        <BarChart
          data={data}
          layout="vertical"  
          margin={{ top: 10, right: 80, left: 40, bottom: 0 }}
          barCategoryGap="85%" 
        >
          <XAxis type="number" />
          <YAxis dataKey="star" type="category" />
          <Tooltip />
          <Bar dataKey="total"  fill="#3586e1"
            radius={[0, 10, 10, 0]} />
        </BarChart>
      </ResponsiveContainer>

   </div>
       <div className="FeedbackListContainer">
        <h1>Feedback Lists</h1>
        <div className="OptionsBox">
            <div className="Searchbar">
                <Image className="SearchImg"src={search} alt=""></Image>
                  <input className="SearchInput" type="text"></input>            
            </div>
          
             <input className="StarAmount" type="number"></input>
        </div>


        <div className="Listing">
          <div className="FeedbackBox" onClick={() => router.push("/AdminFeedbacks/FeedbackDetails")}>
          <div className="Email">Email123456789@gmail.com</div>
          <div className="AmountStars">
             <Image src={Star} alt=""></Image>
            <Image src={Star} alt=""></Image>
            <Image src={Star} alt=""></Image>
            <Image src={Star} alt=""></Image>
            <Image src={Star} alt=""></Image>
          </div>
          </div>

          <div className="FeedbackBox"  onClick={() => router.push("/AdminFeedbacks/FeedbackDetails")}>
          <div className="Email">Email123456789@gmail.com</div>
          <div className="AmountStars">
            <Image src={Star} alt=""></Image>
            <Image src={Star} alt=""></Image>
            <Image src={Star} alt=""></Image>
            <Image src={Star} alt=""></Image>



          </div>
          </div>

         <div className="FeedbackBox"  onClick={() => router.push("/AdminFeedbacks/FeedbackDetails")}>
          <div className="Email">Email123456789@gmail.com</div>
          <div className="AmountStars">
            <Image src={Star} alt=""></Image>
            <Image src={Star} alt=""></Image>
          </div>
          </div>
        

           <div className="FeedbackBox"  onClick={() => router.push("/AdminFeedbacks/FeedbackDetails")}>
          <div className="Email">Email123456789@gmail.com</div>
          <div className="AmountStars">
                    <Image src={Star} alt=""></Image>
          </div>
          </div>
        
          {/* 
             Each box

           <div className="FeedbackBox"  onClick={() => router.push("/AdminFeedbacks/FeedbackDetails")}>
          <div className="Email">Email123456789@gmail.com</div>
          <div className="AmountStars">
                    <Image src={Star} alt=""></Image>
          </div>
          </div>
        
          */}

  {feedbacks.map((fb) => (
              <div
                key={fb.id}
                className="FeedbackBox"
                onClick={() =>
                  router.push(`/AdminFeedbacks/FeedbackDetails/${fb.id}`)
                }
              >
                {/* 🔄 REPLACED: email → subject */}
                <div className="Subject">{fb.subject}</div>

                <div className="AmountStars">
                 {[...Array(fb.rating)].map((_, i) => (
          <Image
             key={i}
              src={Star}
             alt=""
             />
                ))}
                </div>
              </div>
            ))}



        </div>

        <div className="Pagination">
              <div className="Left">
        <Image src={Way} alt=""></Image>
         </div>
         <div className="Page">
             <span>3</span>
         </div>
         <div className="Right">
       <Image src={Way} alt=""></Image>
        </div> 
    </div>


       </div>


  </div>

  
  
  </>
  )
}