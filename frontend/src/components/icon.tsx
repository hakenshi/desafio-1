import { User2Icon } from "lucide-react"
import { Avatar, AvatarFallback, AvatarImage } from "./ui/avatar"
import { cn } from "@/lib/utils"

interface Props {
    src?: string
    size?: "sm" | "lg"
}

const sizeClasses = {
    sm: {
        avatar: "h-10 w-10",
        fallback: "p-2",
        icon: "h-5 w-5"
    },
    lg: {
        avatar: "h-24 w-24",
        fallback: "p-4",
        icon: "h-12 w-12"
    }
}

export default function Icon({ src, size = "sm" }: Props) {
    const styles = sizeClasses[size]

    return (
        <Avatar className={styles.avatar}>
            <AvatarImage src={src} />
            <AvatarFallback>
                <div className={cn("rounded-full", styles.fallback)}>
                    <User2Icon 
                        strokeWidth={0} 
                        fill="#6a7282" 
                        className={cn("text-gray-500", styles.icon)} 
                    />
                </div>
            </AvatarFallback>
        </Avatar>
    )
}