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

const [selectedStar, setSelectedStar] = useState<number | null>(null);


const [currentPage, setCurrentPage] = useState(1);
const itemsPerPage = 12;

const filteredFeedbacks = feedbacks.filter((fb) => {
  if (!selectedStar) return true;
  return fb.rating === selectedStar;
});


const totalPages = Math.ceil(filteredFeedbacks.length / itemsPerPage);

const paginatedFeedbacks = filteredFeedbacks.slice(
  (currentPage - 1) * itemsPerPage,
  currentPage * itemsPerPage
);

const [chartData, setChartData] = useState([
  { star: "5", total: 0 },
  { star: "4", total: 0 },
  { star: "3", total: 0 },
  { star: "2", total: 0 },
  { star: "1", total: 0 },
]);

type Feedback = {
  id: number;
  email: string;
  rating: number;
};

useEffect(() => {
  const fetchFeedbacks = async () => {
    try {
      const token = localStorage.getItem("accessToken");
      globalApiClient.setToken(token);

      const res = await globalApiClient.get<Feedback[]>("Feedbacks");

      const feedbackArray = Array.isArray(res.data) ? res.data : [];
      setFeedbacks(feedbackArray);

  
      const counts = { 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 };

      feedbackArray.forEach((fb) => {
        if (fb.rating >= 1 && fb.rating <= 5) {
          counts[fb.rating as 1 | 2 | 3 | 4 | 5]++;
        }
      });

      const formatted = [
        { star: "5", total: counts[5] },
        { star: "4", total: counts[4] },
        { star: "3", total: counts[3] },
        { star: "2", total: counts[2] },
        { star: "1", total: counts[1] },
      ];

      setChartData(formatted);

    } catch (err) {
      console.error(err);
    }
  };

  fetchFeedbacks();
}, []);

useEffect(() => {
  setCurrentPage(1);
}, [selectedStar]);
  
  return ( 
  <>
  <div className="AdminFeedbacksContainer"> 
   <div className="TotalStarContainer">
    <h2>Stars Chart</h2>
    <ResponsiveContainer>
        <BarChart
          data={chartData}
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
          
             <input  className="StarAmount"
  type="number"
  min={1}
  max={5}
  onChange={(e) => {
    const value = Number(e.target.value);
    setSelectedStar(value >= 1 && value <= 5 ? value : null);
  }}></input>
        </div>


        <div className="Listing">
       
          {/* 
             Each box

           <div className="FeedbackBox"  onClick={() => router.push("/AdminFeedbacks/FeedbackDetails")}>
          <div className="Email">Email123456789@gmail.com</div>
          <div className="AmountStars">
                    <Image src={Star} alt=""></Image>
          </div>
          </div>
        
          */}

  {paginatedFeedbacks.map((fb) => (
  <div
    key={fb.id}
    className="FeedbackBox"
    onClick={() =>
      router.push(`/AdminFeedbacks/FeedbackDetails/${fb.id}`)
    }
  >
    <div className="Email">
      {fb.email?.trim() ? fb.email : "Anonymous"}
    </div>

    <div className="AmountStars">
      {[...Array(fb.rating)].map((_, i) => (
        <Image key={i} src={Star} alt="" />
      ))}
    </div>
  </div>
))}


        </div>

       <div className="Pagination">
  <div
    className="Left"
    onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
  >
    <Image src={Way} alt="" />
  </div>

  <div className="Page">
    <span>{currentPage}</span>
  </div>

  <div
    className="Right"
    onClick={() =>
      setCurrentPage((prev) => Math.min(prev + 1, totalPages))
    }
  >
    <Image src={Way} alt="" />
  </div>
</div>

       </div>


  </div>

  
  
  </>
  )
}