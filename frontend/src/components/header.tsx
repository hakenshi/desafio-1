"use client";

import { LogOut, Settings, UserIcon, EllipsisIcon } from "lucide-react";
import Icon from "./icon";
import { actions } from "@/server/controllers";
import Link from "next/link";
import { Dialog, DialogContent, DialogHeader } from "./ui/dialog";
import { DialogTitle } from "@radix-ui/react-dialog";
import UserCard from "./user-card";
import { AuthModel } from "@/server/models/auth.model";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "./ui/dropdown-menu";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { ThemeToggle } from "./theme-toggle";
import { Logo } from "./logo";

interface Props {
  user: AuthModel.UserInfo;
}

export function Header({ user }: Props) {
  const router = useRouter();
  const [showUserDetails, setShowUserDetails] = useState(false);
  const [isLoggingOut, setIsLoggingOut] = useState(false);

  const handleLogout = async () => {
    setIsLoggingOut(true);
    try {
      await actions.auth.logout();
      router.push("/login");
      router.refresh();
    } catch (error) {
      console.error("Logout failed:", error);
    } finally {
      setIsLoggingOut(false);
    }
  };

  return (
    <header className="w-full bg-card border-b col-span-2 lg:col-span-2 row-span-1 sticky top-0 z-30">
      <nav className="flex justify-between items-center px-4 py-3 mx-auto">
        <Link href="/dashboard" className="text-center flex pl-10 lg:pl-0">
          <Logo width={100} height={10} />
        </Link>

        <div className="flex gap-2 items-center">
          <div className="flex gap-2 border-r border-border px-2 items-center">
            <ThemeToggle />
          </div>

          <Icon />
          <div className="text-start hidden sm:block">
            <p className="text-foreground text-sm">{user.firstName}</p>
            <p className="text-xs text-muted-foreground">{user.role}</p>
          </div>

          <DropdownMenu>
            <DropdownMenuTrigger className="cursor-pointer p-1">
              <EllipsisIcon className="text-foreground h-5 w-5" />
            </DropdownMenuTrigger>
            <DropdownMenuContent className="mr-2 mt-2 p-2" align="end">
              <div className="sm:hidden px-2 py-1.5 mb-2 border-b">
                <p className="text-sm font-medium">{user.firstName}</p>
                <p className="text-xs text-muted-foreground">{user.role}</p>
              </div>
              <DropdownMenuItem onSelect={() => setShowUserDetails(true)}>
                <UserIcon className="mr-2 h-4 w-4" /> User Details
              </DropdownMenuItem>
              <DropdownMenuItem onSelect={() => router.push("/settings")}>
                <Settings className="mr-2 h-4 w-4" /> Settings
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                onSelect={handleLogout}
                disabled={isLoggingOut}
                className="text-destructive"
              >
                <LogOut className="mr-2 h-4 w-4" />{" "}
                {isLoggingOut ? "Logging out..." : "Logout"}
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>

          <Dialog open={showUserDetails} onOpenChange={setShowUserDetails}>
            <DialogContent className="max-w-[95vw] sm:max-w-md">
              <DialogHeader className="sr-only">
                <DialogTitle>User Card</DialogTitle>
              </DialogHeader>
              <UserCard user={user} />
            </DialogContent>
          </Dialog>
        </div>
      </nav>
    </header>
  );
}
