"use client"

import { AuthModel } from "@/server/models/auth.model"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "../ui/form"
import { Input } from "../ui/input"
import { Button } from "../ui/button"
import { actions } from "@/server/controllers"
import Image from "next/image"

export function LoginForm() {
    const form = useForm<AuthModel.LoginRequest>({
        resolver: zodResolver(AuthModel.LoginRequestSchema),
        defaultValues: {
            email: "",
            password: ""
        }
    })

    const submit = async (values: AuthModel.LoginRequest) => {
        await actions.auth.login(values)
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(submit)} className="grid border border-gray-300 p-5 max-w-xl w-full h-100 bg-white rounded-2xl">
                <div>
                    <Image
                        width={300}
                        height={100}
                        src="/logo.svg"
                        alt="hypesoft logo"
                    />
                    <p className="text-gray-500 text-center">Enter your credentials to access the dashboard</p>
                </div>

                <div className="space-y-8">
                    <FormField
                        control={form.control}
                        name="email"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Email</FormLabel>
                                <FormControl>
                                    <Input placeholder="example@email.com" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                    <FormField
                        control={form.control}
                        name="password"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Password</FormLabel>
                                <FormControl>
                                    <Input type="password" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                </div>
                <div className="flex items-end justify-end">
                    <Button type="submit">
                        Login
                    </Button>
                </div>
            </form>
        </Form>
    )

}