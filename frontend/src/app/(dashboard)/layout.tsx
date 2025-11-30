import { Header } from "@/components/header";
import Sidebar from "@/components/sidebar";
import { PropsWithChildren } from "react";

export default function DashboardLayout({ children }: PropsWithChildren) {
    return (
        <div className="grid h-screen grid-rows-[auto_1fr] grid-cols-[12rem_auto]">
            <Header />
            <Sidebar />
            <main className="row-start-2 col-start-2 overflow-hidden">
                {children}
            </main>
        </div>
    )
}