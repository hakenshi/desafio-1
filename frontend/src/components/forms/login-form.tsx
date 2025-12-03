"use client"

import { AuthModel } from "../../server/models/auth.model"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "../ui/form"
import { Input } from "../ui/input"
import { Button } from "../ui/button"
import { actions } from "../../server/controllers"
import Image from "next/image"
import { Spinner } from "../ui/spinner"

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
            <form onSubmit={form.handleSubmit(submit)} className="grid gap-6 border border-border p-8 max-w-xl w-full bg-card rounded-2xl">
                <div className="flex flex-col items-center gap-5">
                    <div>
                        <Image
                            width={150}
                            height={10}
                            priority
                            src="/logo.svg"
                            alt="hypesoft logo"
                            className="dark:hidden"
                        />
                        <Image
                            width={150}
                            height={10}
                            priority
                            src="/logo-dark.svg"
                            alt="hypesoft logo"
                            className="hidden dark:block"
                        />
                    </div>
                    <p className="text-muted-foreground">Enter your credentials to access the dashboard</p>
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