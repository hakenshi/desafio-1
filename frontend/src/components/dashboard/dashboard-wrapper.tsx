"use client";

import { PropsWithChildren, useState, createContext, useContext } from "react";

interface SidebarContextType {
  isOpen: boolean;
  toggle: () => void;
  close: () => void;
}

const SidebarContext = createContext<SidebarContextType>({
  isOpen: false,
  toggle: () => {},
  close: () => {},
});

export const useSidebar = () => useContext(SidebarContext);

export function DashboardWrapper({ children }: PropsWithChildren) {
  const [sidebarOpen, setSidebarOpen] = useState(false);

  return (
    <SidebarContext.Provider
      value={{
        isOpen: sidebarOpen,
        toggle: () => setSidebarOpen(!sidebarOpen),
        close: () => setSidebarOpen(false),
      }}
    >
      <div className="min-h-screen bg-background">
        <div className="lg:grid lg:grid-rows-[auto_1fr] lg:grid-cols-[12rem_1fr] lg:h-screen">
          {children}
        </div>
      </div>
    </SidebarContext.Provider>
  );
}
