import { NextResponse } from "next/server";

export async function POST(req: Request) {
  try {
    const { phone, amount, coupon } = await req.json();

    if (!phone || !amount) {
      return NextResponse.json(
        { message: "Missing data" },
        { status: 400 }
      );
    }

    // 👉 giả lập xử lý
    await new Promise((res) => setTimeout(res, 1000));

    return NextResponse.json({
      success: true,
      transactionId: Date.now(),
      phone,
      amount,
    });

  } catch (error) {
    return NextResponse.json(
      { message: "Server error" },
      { status: 500 }
    );
  }
}