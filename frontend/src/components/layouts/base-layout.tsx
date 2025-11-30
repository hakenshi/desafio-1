import { AuthService } from "@/server/services";
import { PropsWithChildren } from "react";

export async function BaseLayout({ children }:PropsWithChildren){
    const session = await AuthService.getUserInfo()
}