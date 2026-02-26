import Section from "@/shared/ui/Section";
import { MainLayout } from "@/shared/layout/MainLayout";

export default function About() {
  return (
    <MainLayout>
      <Section className="py-20">
        <div className="container mx-auto px-6">
          <h1 className="text-4xl font-bold mb-6">About BarberBooking</h1>
          <p className="text-gray-600 max-w-2xl">
            BarberBooking makes it effortless to schedule your next cut with
            smart availability, instant confirmation, and helpful reminders.
          </p>
        </div>
      </Section>
    </MainLayout>
  );
}
