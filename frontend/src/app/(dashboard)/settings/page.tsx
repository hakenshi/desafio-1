import { Suspense } from "react";
import DashboardShell from "@/components/dashboard/dashboard-shell";
import { getUserInfo } from "@/server/controllers/auth.controller";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { TableSkeleton } from "@/components/dashboard/table-skeleton";
import SettingsForm from "@/components/forms/settings-form";
import ThemeSettings from "@/components/settings/theme-settings";

async function SettingsContent() {
  const user = await getUserInfo();

  return (
    <div className="grid gap-6 md:grid-cols-2">
      <Card>
        <CardHeader>
          <CardTitle>Profile Information</CardTitle>
          <CardDescription>
            Update your personal information and email address.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <SettingsForm user={user} />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Appearance</CardTitle>
          <CardDescription>
            Customize the appearance of the application.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <ThemeSettings />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Account Information</CardTitle>
          <CardDescription>
            View your account details.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <p className="text-sm font-medium text-muted-foreground">Username</p>
            <p className="text-sm">{user.username}</p>
          </div>
          <div>
            <p className="text-sm font-medium text-muted-foreground">Role</p>
            <p className="text-sm capitalize">{user.role}</p>
          </div>
          <div>
            <p className="text-sm font-medium text-muted-foreground">User ID</p>
            <p className="text-sm font-mono text-xs">{user.id}</p>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

export default function SettingsPage() {
  return (
    <DashboardShell title="Settings">
      <Suspense fallback={<TableSkeleton columns={2} rows={3} />}>
        <SettingsContent />
      </Suspense>
    </DashboardShell>
  );
}
