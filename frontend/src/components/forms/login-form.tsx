"use client"

import { AuthModel } from "../../server/models/auth.model"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "../ui/form"
import { Input } from "../ui/input"
import { Button } from "../ui/button"
import { actions } from "../../server/controllers"
import Image from "next/image"
import { redirect } from "next/navigation"
import { Spinner } from "../ui/spinner"

export function LoginForm() {
    const form = useForm<AuthModel.LoginRequest>({
        resolver: zodResolver(AuthModel.LoginRequestSchema),
        defaultValues: {
            email: "admin@hypesoft.com",
            password: "admin123"
        }
    })

    const submit = async (values: AuthModel.LoginRequest) => {
        await actions.auth.login(values)
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(submit)} className="grid gap-6 border border-gray-300 p-8 max-w-xl w-full bg-white rounded-2xl">
                <div className="flex flex-col items-center gap-5">
                    <div>
                        <Image
                            width={75}
                            height={10}
                            priority
                            src="/logo.svg"
                            alt="hypesoft logo"
                        />
                    </div>
                    <p className="text-gray-500">Enter your credentials to access the dashboard</p>
                </div>

                <div className="space-y-6">
                    <FormField
                        control={form.control}
                        name="email"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Email</FormLabel>
                                <FormControl>
                                    <Input type="email" placeholder="example@email.com" {...field} />
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

                <Button disabled={form.formState.isSubmitting} type="submit" className="w-full">
                    {form.formState.isSubmitting ? (<> <Spinner /> Login </>) : "Login"}
                </Button>
            </form>
        </Form>
    )
}