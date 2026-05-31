import { ServicePlanList } from "@/features/services/ServicePlanList";

export default function DataPlanPage() {
  return (
    <section className="fabu-section">
      <div className="fabu-container">
        <div className="mb-8">
          <h1>5G Data Plans</h1>
          <p className="mt-2 text-sm text-fabu-gray">
            Live service packages from the backend `Service` REST endpoint.
          </p>
        </div>
        <ServicePlanList />
      </div>
    </section>
  );
}
