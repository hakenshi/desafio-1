"use client";

import { actions } from "@/server/controllers";
import { CategoryModel } from "@/server/models/category.model";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import {
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from "../ui/form";
import { Input } from "../ui/input";
import { Button } from "../ui/button";
import { Spinner } from "../ui/spinner";
import { Textarea } from "../ui/textarea";
import { useRouter } from "next/navigation";

interface Props {
    category?: CategoryModel.Category;
    onSuccess?: () => void;
}

export default function CategoryForm({ category, onSuccess }: Props) {
    const isUpdating = !!category;
    const router = useRouter();

    const form = useForm<CategoryModel.CreateCategoryDto>({
        resolver: zodResolver(CategoryModel.CreateCategorySchema),
        defaultValues: category
            ? {
                name: category.name,
                description: category.description,
            }
            : {
                name: "",
                description: "",
            },
    });

    const submit = async (values: CategoryModel.CreateCategoryDto) => {
        if (isUpdating) {
            await actions.category.updateCategory(category.id, values);
        } else {
            await actions.category.createCategory(values);
        }
        router.refresh();
        onSuccess?.();
    };

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(submit)} className="space-y-4">
                <FormField
                    control={form.control}
                    name="name"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Name</FormLabel>
                            <FormControl>
                                <Input placeholder="Category name" {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="description"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Description</FormLabel>
                            <FormControl>
                                <Textarea
                                    placeholder="Category description"
                                    className="resize-none"
                                    {...field}
                                />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <Button
                    disabled={form.formState.isSubmitting}
                    type="submit"
                    className="w-full"
                >
                    {form.formState.isSubmitting ? (
                        <>
                            <Spinner /> {isUpdating ? "Updating..." : "Creating..."}
                        </>
                    ) : isUpdating ? (
                        "Update Category"
                    ) : (
                        "Create Category"
                    )}
                </Button>
            </form>
        </Form>
    );
}
