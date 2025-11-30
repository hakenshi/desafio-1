import Image from "next/image";
import { Input } from "./ui/input";
import { BellIcon, SearchIcon, SunIcon } from "lucide-react";
import Icon from "./ui/icon";
import { actions } from "@/server/controllers";

export async function Header() {

    const user = await actions.auth.getUserInfo()
    
    return (
        <header className="w-full bg-white">
            <nav className="flex justify-between items-center px-5 py-3 mx-auto">
                <div className="text-center ">
                    <Image
                        width={25}
                        height={10}
                        priority
                        src="/logo.svg"
                        alt="hypesoft logo"
                    />
                </div>
                <div className="relative max-w-md w-full">
                    <SearchIcon className="absolute top-1/2 -translate-y-1/2 left-3 text-gray-400" size={18} />
                    <Input placeholder="Search" className="pl-10 bg-gray-100 rounded-full border-none w-full" />
                </div>
                <div className="flex gap-2 items-center">
                    <div className="flex gap-2 border-r border-gray-300 px-2">
                        <SunIcon fill="black" />
                        <BellIcon fill="black" />
                    </div>
                    <div>
                        <Icon />
                    </div>
                </div>
            </nav>
        </header>
    )
}