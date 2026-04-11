"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { globalApiClient } from "@/app/api/ApiClient";
import { serviceSchema } from "@/core/validations/AddService";
export default function AdminPackages() {

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

  const [errors, setErrors] = useState<any>({});
  const [packages, setPackages] = useState<Package[]>([]);
  const router = useRouter(); 


  const [form, setForm] = useState({
    serviceName: "",
    serviceCode: "",
    category: "",
    dataAmountMB: "",
    price: "",
    validityDays: "",
    description: "",
    maxActivationsPerMonth: "",
  });

 
  const handleChange = (e: any) => {
    setForm({
      ...form,
      [e.target.name]: e.target.value,
    });
  };


  const handleSubmit = async () => {
  try {

    const result = serviceSchema.safeParse(form);

    if (!result.success) {
      const fieldErrors: any = {};

      result.error.issues.forEach((err) => {
        fieldErrors[err.path[0]] = err.message;
      });

      setErrors(fieldErrors);
      return;
    }

    setErrors({}); 

    const token = localStorage.getItem("accessToken");
    globalApiClient.setToken(token);

    const payload = {
      ...result.data,
      isActive: true,
    };

    const res = await globalApiClient.post<any>("Service", payload);

    router.push("/AdminPackages");

  } catch (err) {
    console.error("CREATE ERROR:", err);
  }
};

  return (
    <div className="AddServForm">
      <h1>Add New Package</h1>

      <div className="Formitself">
        <input name="serviceName" value={form.serviceName} placeholder="ServiceName" onChange={handleChange} />
        {errors.serviceName && <span className="error">{errors.serviceName}</span>}
        <input name="serviceCode" value={form.serviceCode} placeholder="ServiceCode" onChange={handleChange} />
        {errors.serviceCode && <span className="error">{errors.serviceCode}</span>}
        <input name="category" value={form.category} placeholder="Category" onChange={handleChange} />
        {errors.category && <span className="error">{errors.category}</span>}
        <input name="dataAmountMB" value={form.dataAmountMB} placeholder="Data Amount(MB)" onChange={handleChange} />
        {errors.dataAmountMB && <span className="error">{errors.dataAmountMB}</span>}
        <input name="price" value={form.price} placeholder="Price" onChange={handleChange} />
        {errors.price && <span className="error">{errors.price}</span>}
        <input name="validityDays" value={form.validityDays} placeholder="Validity (Days)" onChange={handleChange} />
        {errors.validityDays && <span className="error">{errors.validityDays}</span>}
        <input name="description" value={form.description} placeholder="Description" onChange={handleChange} />
        {errors.description && <span className="error">{errors.description}</span>}
        <input name="maxActivationsPerMonth" value={form.maxActivationsPerMonth} placeholder="Max Activation per Month" onChange={handleChange} />
        {errors.maxActivationsPerMonth && <span className="error">{errors.maxActivationsPerMonth}</span>}
      </div>

      <button className="SubmitAdd" onClick={handleSubmit}>
        Add
      </button>
    </div>
  );
}