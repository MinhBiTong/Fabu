"use client";

import Image from "next/image";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { globalApiClient } from "@/app/api/ApiClient";
import Arrow from "../../styles/images/upward-arrow.png";
export default function DataPlan() {
   const router = useRouter()

  type Package = {
    id: number;
    serviceName: string;
    serviceCode: string;
    category: string;
    dataAmountMB: number;
    price: number;
    validityDays: number;
    description: string;
    isActive: boolean;
    maxActivationsPerMonth: number;
  };
const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
   const [packages, setPackages] = useState<Package[]>([]);
const [categories, setCategories] = useState<string[]>([]);

const [selectedCategory, setSelectedCategory] = useState<string>("All");
const [expandedCategories, setExpandedCategories] = useState<string[]>([]);

const toggleSort = () => {
  setSortOrder((prev) => (prev === "asc" ? "desc" : "asc"));
};

const groupedPackages = packages.reduce((acc, pkg) => {
  if (!acc[pkg.category]) acc[pkg.category] = [];

  acc[pkg.category].push(pkg);
  return acc;
}, {} as Record<string, Package[]>);

// 🔥 SORT HERE
Object.keys(groupedPackages).forEach((category) => {
  groupedPackages[category].sort((a, b) =>
    sortOrder === "asc" ? a.price - b.price : b.price - a.price
  );
});

const toggleViewAll = (category: string) => {
  setExpandedCategories((prev) =>
    prev.includes(category)
      ? prev.filter((c) => c !== category)
      : [...prev, category]
  );
};

useEffect(() => {
  const fetchPackages = async () => {
    try {
       const token = localStorage.getItem("accessToken");
        globalApiClient.setToken(token);

     const res = await globalApiClient.get<Package[]>("Service");


     console.log("DATA:", res.data);

             setPackages(res.data);


      const uniqueCategories = [...new Set(res.data.map((p: Package) => p.category))];
      setCategories(uniqueCategories);

    } catch (err) {
      console.error(err);
    }
  };

  fetchPackages();
}, []);
  return (
    
    <div className="p5GContainer">
    <h1>5G Data Plans</h1>
<div className="ChoosePackage">

 <div className="MobilePackageType" onClick={() => {
  setSelectedCategory("All");
  setExpandedCategories([]); 
}}>
  All
</div>

{categories.map((cat) => (
  <div
    key={cat}
    className="MobilePackageType"
    onClick={() => {
  setSelectedCategory(cat);
  setExpandedCategories([cat]); // auto expand this category
}}
  >
    {cat}
  </div>
))}
</div>
    <div className="ValueOptions">
     <div className="Opted" onClick={toggleSort} style={{ cursor: "pointer" }}>
  <p>Price</p>
  <Image
    src={Arrow}
    alt="Price"
    style={{
      transform: sortOrder === "desc" ? "rotate(180deg)" : "rotate(0deg)",
      transition: "0.3s",
    }}
  />
</div>
 
    </div>
     <div className="PackageContain">
  {Object.entries(groupedPackages)
    .filter(([category]) =>
      selectedCategory === "All" || category === selectedCategory
    )
    .map(([category, pkgs]) => {
      const isExpanded = expandedCategories.includes(category);
      const visiblePackages = isExpanded ? pkgs : pkgs.slice(0, 3);

      return (
        <div key={category}>
          {/* Header */}
          <div className="PackageandViewAll">
            <h2>Service Set : {category}</h2>
            <h3 onClick={() => toggleViewAll(category)}>
              {isExpanded ? "Show Less" : "View All"}
            </h3>
          </div>

          {/* Packages */}
          <div className="PackageSet">
            {visiblePackages.map((pkg) => (
              <div key={pkg.id} className="Package">
                <div className="PackageTitle">
                  <h2>{pkg.serviceName}</h2>
                </div>

                <div className="PackageOffer">
                  <p>{pkg.dataAmountMB} MB</p>
                  <p>{pkg.validityDays} days</p>
                </div>

                <div className="PackagePrice">
                  <p>{pkg.price.toLocaleString()} VND</p>
                </div>

                <div className="PackageChoices">
                  <button>Subscribe</button>
                  <h3
                    onClick={() =>
                      router.push(`/P5GDataPlan/Details/${pkg.id}`)
                    }
                  >
                    View Details
                  </h3>
                </div>
              </div>
            ))}
          </div>
        </div>
      );
    })}
</div>

    </div>


    
  );
}