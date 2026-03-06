"use client"; // Bắt buộc vì có xử lý logic form (Interactive)

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { contactSchema, ContactFormValues } from "@/core/validations/contact.schema";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { 
  Form, FormControl, FormField, FormItem, FormLabel, FormMessage 
} from "@/components/ui/form";

export function ContactForm() {
  // 1. Khởi tạo Form (giống cách khai báo Model trong .NET)
  const form = useForm<ContactFormValues>({
    resolver: zodResolver(contactSchema),
    defaultValues: { fullName: "", email: "", subject: "", message: "" },
  });

  // 2. Xử lý Submit
  function onSubmit(values: ContactFormValues) {
    console.log("Dữ liệu gửi sang .NET Backend:", values);
    alert("Cảm ơn bạn! Chúng tôi sẽ phản hồi sớm.");
    form.reset();
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Gửi tin nhắn cho chúng tôi</CardTitle>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="fullName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Họ và tên</FormLabel>
                  <FormControl><Input placeholder="Nguyễn Văn A" {...field} /></FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Email</FormLabel>
                  <FormControl><Input type="email" placeholder="email@example.com" {...field} /></FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="message"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Nội dung</FormLabel>
                  <FormControl><Textarea rows={4} placeholder="Nhập nội dung cần hỗ trợ..." {...field} /></FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <Button type="submit" className="w-full">Gửi yêu cầu</Button>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}