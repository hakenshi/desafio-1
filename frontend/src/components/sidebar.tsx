"use client";

import {
  HomeIcon,
  Settings,
  ShoppingBag,
  StoreIcon,
  Users,
  Menu,
  X,
} from "lucide-react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { AuthModel } from "@/server/models/auth.model";
import { Button } from "./ui/button";
import { cn } from "@/lib/utils";
import { useSidebar } from "./dashboard/dashboard-wrapper";

interface Props {
  user: AuthModel.UserInfo;
}

const navItems = [
  {
    section: "General",
    items: [{ href: "/dashboard", icon: HomeIcon, label: "Dashboard" }],
  },
  {
    section: "Shop",
    items: [
      { href: "/products", icon: ShoppingBag, label: "Products" },
      { href: "/categories", icon: StoreIcon, label: "Categories" },
      { href: "/users", icon: Users, label: "Users", adminOnly: true },
    ],
  },
  {
    section: "Support",
    items: [{ href: "/settings", icon: Settings, label: "Settings" }],
  },
];

export default function Sidebar({ user }: Props) {
  const pathname = usePathname();
  const { isOpen, toggle, close } = useSidebar();
  const isAdmin = user.role === "admin";

  return (
    <>
      {isOpen && (
        <div
          className="fixed inset-0 bg-black/50 z-40 lg:hidden"
          onClick={close}
        />
      )}

      <Button
        variant="ghost"
        size="icon"
        className="lg:hidden fixed top-3 left-3 z-50"
        onClick={toggle}
      >
        {isOpen ? <X className="h-5 w-5" /> : <Menu className="h-5 w-5" />}
      </Button>

      <aside
        className={cn(
          "bg-card border-r fixed lg:static inset-y-0 left-0 z-40 w-56 lg:w-48 transform transition-transform duration-200 ease-in-out",
          "lg:transform-none lg:row-start-2 lg:col-start-1",
          isOpen ? "translate-x-0" : "-translate-x-full lg:translate-x-0"
        )}
      >
        <nav className="space-y-6 px-4 py-4 pt-16 lg:pt-4 h-full overflow-y-auto">
          {navItems.map((section) => (
            <div key={section.section}>
              <p className="text-muted-foreground font-semibold text-xs uppercase tracking-wider">
                {section.section}
              </p>
              <ul className="pt-2 space-y-1">
                {section.items.map((item) => {
                  if (item.adminOnly && !isAdmin) return null;
                  const isActive = pathname === item.href;
                  return (
                    <li key={item.href}>
                      <Link
                        href={item.href}
                        onClick={close}
                        className={cn(
                          "flex gap-3 items-center p-2 rounded-md text-sm transition-colors",
                          isActive
                            ? "bg-primary text-primary-foreground"
                            : "text-foreground hover:bg-accent hover:text-primary"
                        )}
                      >
                        <item.icon className="h-4 w-4" />
                        <span>{item.label}</span>
                      </Link>
                    </li>
                  );
                })}
              </ul>
            </div>
          ))}
        </nav>
      </aside>
    </>
  );
}
