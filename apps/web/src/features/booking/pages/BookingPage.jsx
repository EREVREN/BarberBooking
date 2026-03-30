import { useLocation } from "react-router-dom";
import BookingProvider from "../BookingProvider";
import BookingStepper from "../BookingStepper";
import { MainLayout } from "@/shared/layout/MainLayout";
import { AiAssistant } from "../assistant/AiAssistant";

export default function BookingPage() {
  const location = useLocation();
  const resetSignal = location.state?.resetAt ?? "default";

  return (
    <MainLayout>
      <div className="min-h-screen bg-gray-50 flex justify-center pt-6 pb-10">
        <div className="w-full max-w-3xl bg-white rounded-xl shadow p-6">
          <h1 className="text-2xl font-semibold mb-4">Book an Appointment</h1>
          <BookingProvider>
            <div className="max-w-3xl mx-auto p-6">
              <BookingStepper resetSignal={resetSignal} />
              <AiAssistant />
            </div>
          </BookingProvider>
        </div>
      </div>
    </MainLayout>
  );
}
