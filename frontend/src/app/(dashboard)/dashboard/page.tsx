import { Suspense } from "react";
import DashboardShell from "@/components/dashboard/dashboard-shell";
import { DashboardStats, DashboardCharts } from "@/components/dashboard/dashboard-content";
import { StatCardsSkeleton, ChartsSkeleton } from "@/components/dashboard/dashboard-skeletons";

export default function DashboardPage() {
  return (
    <DashboardShell title="Dashboard">
      <Suspense fallback={<StatCardsSkeleton />}>
        <DashboardStats />
      </Suspense>
      
      <Suspense fallback={<ChartsSkeleton />}>
        <DashboardCharts />
      </Suspense>
    </DashboardShell>
  );
}
