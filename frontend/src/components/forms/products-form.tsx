"use client";

import { actions } from "@/server/controllers";
import { ProductModel } from "@/server/models/product.model";
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
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "../ui/select";
import { Textarea } from "../ui/textarea";
import { useRouter } from "next/navigation";
import { useQuery } from "@tanstack/react-query";

interface Props {
    product?: ProductModel.Product;
    onSuccess?: () => void;
}

export default function ProductsForm({ product, onSuccess }: Props) {
    const isUpdating = !!product;
    const router = useRouter();
    
    const { data: categoriesData, isLoading: isLoadingCategories } = useQuery({
        queryKey: ["categories"],
        queryFn: () => actions.category.getCategories({ page: 1, pageSize: 100 }),
    });
    
    const categories = categoriesData?.items ?? [];

    const form = useForm<ProductModel.CreateProductDto>({
        resolver: zodResolver(ProductModel.CreateProductSchema),
        defaultValues: product
            ? {
                categoryId: product.categoryId,
                description: product.description,
                name: product.name,
                price: product.price,
                stockQuantity: product.stockQuantity,
            }
            : {
                name: "",
                description: "",
                price: 0,
                categoryId: "",
                stockQuantity: 0,
            },
    });

    const submit = async (values: ProductModel.CreateProductDto) => {
        try {
            if (isUpdating) {
                await actions.product.updateProduct(product.id, values);
            } else {
                await actions.product.createProduct(values);
            }
            router.refresh();
            onSuccess?.();
        } catch (error) {
            console.error("Failed to save product:", error);
        }
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
                                <Input placeholder="Product name" {...field} />
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
                                    placeholder="Product description"
                                    className="resize-none"
                                    {...field}
                                />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <div className="grid grid-cols-2 gap-4">
                    <FormField
                        control={form.control}
                        name="price"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Price</FormLabel>
                                <FormControl>
                                    <Input
                                        type="number"
                                        step="0.01"
                                        min="0"
                                        placeholder="0.00"
                                        {...field}
                                        onChange={(e) => field.onChange(parseFloat(e.target.value) || 0)}
                                    />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <FormField
                        control={form.control}
                        name="stockQuantity"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Stock Quantity</FormLabel>
                                <FormControl>
                                    <Input
                                        type="number"
                                        min="0"
                                        placeholder="0"
                                        {...field}
                                        onChange={(e) => field.onChange(parseInt(e.target.value) || 0)}
                                    />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                </div>

                <FormField
                    control={form.control}
                    name="categoryId"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Category</FormLabel>
                            <Select onValueChange={field.onChange} defaultValue={field.value} disabled={isLoadingCategories}>
                                <FormControl>
                                    <SelectTrigger>
                                        <SelectValue placeholder={isLoadingCategories ? "Loading..." : "Select a category"} />
                                    </SelectTrigger>
                                </FormControl>
                                <SelectContent className="w-full">
                                    {categories.map((category) => (
                                        <SelectItem key={category.id} value={category.id}>
                                            {category.name}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
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
                        "Update Product"
                    ) : (
                        "Create Product"
                    )}
                </Button>
            </form>
        </Form>
    );
}
