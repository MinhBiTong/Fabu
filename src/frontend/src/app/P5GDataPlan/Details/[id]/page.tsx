import { ServicePlanDetailsView } from "@/features/services/ServicePlanDetailsView";

type DataPlanDetailsPageProps = {
  params: Promise<{ id: string }>;
};

export default async function DataPlanDetailsPage({ params }: DataPlanDetailsPageProps) {
  const { id } = await params;

  return (
    <section className="fabu-section">
      <div className="fabu-container">
        <ServicePlanDetailsView id={id} />
      </div>
    </section>
  );
}
