"use client";

import Image from "next/image";
import Way from "../../styles/images/Way.png";

import { useEffect, useState } from "react";
import { globalApiClient } from "@/app/api/ApiClient";
import { useRouter } from "next/navigation";

export default function AdminPackages() {
  const router = useRouter();


  const [packages, setPackages] = useState<Package[]>([]);
const [searchTerm, setSearchTerm] = useState("");
  
    
const [selectedCategory, setSelectedCategory] = useState("");
const categories = [...new Set(packages.map(p => p.category))];

const filteredPackages = packages.filter((pkg) => {
  const matchSearch = pkg.serviceName
    .toLowerCase()
    .includes(searchTerm.toLowerCase());

  const matchCategory =
    selectedCategory === "" || pkg.category === selectedCategory;

  return matchSearch && matchCategory;
});


  const [currentPage, setCurrentPage] = useState(1);

    const itemsPerPage = 10;
  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentPackages = filteredPackages.slice(indexOfFirstItem, indexOfLastItem);


  const totalPages = Math.ceil(filteredPackages.length / itemsPerPage);


  type Package = {
    id: number;
    serviceName: string;
    category: string;
    dataAmountMB: number;
    price: number;
    validityDays: number;
  };
  
  useEffect(() => {
    const fetchPackages = async () => {
      try {
        const token = localStorage.getItem("accessToken");
        globalApiClient.setToken(token);

        const res = await globalApiClient.get<Package[]>("Service");

         console.log("DATA:", res.data);

             setPackages(res.data);
      } catch (err) {
        console.error(err);
      }
    };

    fetchPackages();
  }, []);


  /* Add PAckage area*/







  return (
    <>
    <div className="PackagesContainer">
      <h1>Packages</h1>

      <div className="SearchTools">
        <input className="Search" placeholder="Search Package Name" value={searchTerm} onChange={(e) => {setSearchTerm(e.target.value); setCurrentPage(1);  }} />
     <select
  className="DropDown"
  value={selectedCategory}
  onChange={(e) => {
    setSelectedCategory(e.target.value);
    setCurrentPage(1); 
  }}
>
  <option value="">All Categories</option>

  {categories.map((cat) => (
    <option key={cat} value={cat}>
      {cat}
    </option>
  ))}
</select>          
      </div>

      <div className="SearchTools">
        <button className="AddPack"  onClick={() =>router.push(`/AdminPackages/AddPackage`)}>Add Package</button>
      </div>

      <div className="TableList">
        <table>
          <thead>
            <tr>
              <th>Package Name</th>
              <th>Amount</th>
              <th>Category</th>
              <th>Price</th>
              <th>Length</th>
              <th>Options</th>
            </tr>
          </thead>

          <tbody>
            {filteredPackages.length === 0 ? (
              <tr>
                <td colSpan={6} style={{ textAlign: "center" }}>
                  No packages found
                </td>
              </tr>
            ) : (
              currentPackages.map((pkg) => (
                <tr key={pkg.id}>
                  <td>{pkg.serviceName}</td>

             
                  <td>{pkg.dataAmountMB} GB</td>

                  <td>{pkg.category}</td>

                  <td>${pkg.price}</td>

                  <td>{pkg.validityDays} Days</td>

                  <td>
                    <span
                      className="Clickablewords"
                      onClick={() =>
                        router.push(`/AdminPackages/PackagesDetails/${pkg.id}`)
                      }
                    >
                      Details
                    </span>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

    <div className="Pagination">
  
   <div
    className="Left"
    onClick={() => {
      if (currentPage > 1) {
        setCurrentPage((prev) => prev - 1);
      }
    }}
    style={{
      opacity: currentPage === 1 ? 0.5 : 1,
      cursor: currentPage === 1 ? "not-allowed" : "pointer",
    }}
  >
    <Image src={Way} alt="left" />
  </div>


  <div className="Page">
  <span>{currentPage}</span>
</div>

  <div
    className="Right"
    onClick={() => {
      if (currentPage < totalPages) {
        setCurrentPage((prev) => prev + 1);
      }
    }}
    style={{
      opacity: currentPage === totalPages ? 0.5 : 1,
      cursor: currentPage === totalPages ? "not-allowed" : "pointer",
    }}
  >
    <Image src={Way} alt="right" />
  </div>
</div>
    </div>

</>
  );
}