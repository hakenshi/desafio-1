'use client'

import { Input } from "./ui/input";
import { BellIcon, EllipsisIcon, LogOut, SearchIcon, Settings, UserIcon } from "lucide-react";
import Icon from "./icon";
import { actions } from "@/server/controllers";
import Link from "next/link";
import { Dialog, DialogContent, DialogHeader } from "./ui/dialog";
import { DialogTitle } from "@radix-ui/react-dialog";
import UserCard from "./user-card";
import { AuthModel } from "@/server/models/auth.model";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from "./ui/dropdown-menu";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { ThemeToggle } from "./theme-toggle";
import { Logo } from "./logo";

interface Props {
    user: AuthModel.UserInfo
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
        <header className="w-full bg-card border-b col-span-2 row-span-1">
            <nav className="flex justify-between items-center px-5 py-3 mx-auto">
                <Link href={"/dashboard"} className="text-center flex">
                    <Logo width={100} height={10} />
                </Link>
                <div className="relative w-1/4">
                    <SearchIcon className="absolute top-1/2 -translate-y-1/2 left-3 text-muted-foreground" size={18} />
                    <Input placeholder="Search" className="pl-10 bg-muted rounded-full border-none w-full" />
                </div>
                <div className="flex gap-2 items-center">
                    <div className="flex gap-2 border-r border-border px-2 items-center">
                        <ThemeToggle />
                        <BellIcon className="h-5 w-5 text-muted-foreground" />
                    </div>
                    <Icon />
                    <div className="text-start">
                        <p className="text-foreground text-sm">{user.firstName}</p>
                        <p className="text-xs text-muted-foreground">{user.role}</p>
                    </div>
                    <DropdownMenu>
                        <DropdownMenuTrigger className="cursor-pointer">
                            <EllipsisIcon className="text-foreground" />
                        </DropdownMenuTrigger>
                        <DropdownMenuContent className="mr-2 mt-5 p-2">
                            <DropdownMenuItem onSelect={() => setShowUserDetails(true)}>
                                <UserIcon /> User Details
                            </DropdownMenuItem>
                            <DropdownMenuItem onSelect={() => router.push("/settings")}>
                                <Settings /> Settings
                            </DropdownMenuItem>
                            <DropdownMenuSeparator />
                            <DropdownMenuItem onSelect={handleLogout} disabled={isLoggingOut} className="text-destructive">
                                <LogOut /> {isLoggingOut ? "Logging out..." : "Logout"}
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                    <Dialog open={showUserDetails} onOpenChange={setShowUserDetails}>
                        <DialogContent>
                            <DialogHeader className="sr-only">
                                <DialogTitle>User Card</DialogTitle>
                            </DialogHeader>
                            <UserCard user={user} />
                        </DialogContent>
                    </Dialog>
                </div>
            </nav>
        </header>
    )
}