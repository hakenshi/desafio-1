"use client";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { ChartContainer, ChartTooltip, ChartTooltipContent } from "@/components/ui/chart";
import { Bar, BarChart, XAxis, YAxis, Pie, PieChart, Cell } from "recharts";
import { Package, DollarSign, AlertTriangle, Layers } from "lucide-react";
import { DashboardModel } from "@/server/models/dashboard.model";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

const COLORS = [
  "#9333ea", // purple-600 (primary)
  "#a855f7", // purple-500
  "#7c3aed", // violet-600
  "#8b5cf6", // violet-500
  "#6366f1", // indigo-500
  "#c084fc", // purple-400
  "#a78bfa", // violet-400
  "#818cf8", // indigo-400
  "#d8b4fe", // purple-300
  "#c4b5fd", // violet-300
  "#a5b4fc", // indigo-300
  "#7e22ce", // purple-700
  "#6d28d9", // violet-700
  "#4f46e5", // indigo-600
  "#581c87", // purple-900
  "#5b21b6", // violet-800
  "#4338ca", // indigo-700
  "#e9d5ff", // purple-200
  "#ddd6fe", // violet-200
  "#c7d2fe", // indigo-200
  "#f3e8ff", // purple-100
  "#ede9fe", // violet-100
  "#e0e7ff", // indigo-100
  "#3730a3", // indigo-800
  "#312e81", // indigo-900
];

interface DashboardProps {
  dashboard: DashboardModel.Dashboard;
}

interface AuditLogsProps {
  logs: DashboardModel.AuditLog[];
}

interface RecentProductsProps {
  products: DashboardModel.RecentProduct[];
}

export function DashboardStats({ dashboard }: DashboardProps) {
  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Total Products</CardTitle>
          <Package className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{new Intl.NumberFormat("en-US").format(dashboard.totalProducts)}</div>
          <p className="text-xs text-muted-foreground">Products in inventory</p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Total Stock Value</CardTitle>
          <DollarSign className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">
            {new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" }).format(dashboard.totalStockValue)}
          </div>
          <p className="text-xs text-muted-foreground">Total inventory value</p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Low Stock Items</CardTitle>
          <AlertTriangle className="h-4 w-4 text-destructive" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold text-destructive">{dashboard.lowStockCount}</div>
          <p className="text-xs text-muted-foreground">Products below 10 units</p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Categories</CardTitle>
          <Layers className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{Object.keys(dashboard.productsByCategory).length}</div>
          <p className="text-xs text-muted-foreground">Product categories</p>
        </CardContent>
      </Card>
    </div>
  );
}

export function DashboardCharts({ dashboard }: DashboardProps) {
  const categoryData = Object.entries(dashboard.productsByCategory)
    .map(([name, value]) => ({ name, value }))
    .sort((a, b) => b.value - a.value)
    .slice(0, 10);

  const chartConfig = categoryData.reduce((acc, item, index) => {
    acc[item.name] = { label: item.name, color: COLORS[index % COLORS.length] };
    return acc;
  }, {} as Record<string, { label: string; color: string }>);

  return (
    <div className="grid gap-4 grid-cols-1 md:grid-cols-2 mt-4">
      <Card className="overflow-hidden">
        <CardHeader className="pb-2">
          <CardTitle className="text-base sm:text-lg">Products by Category</CardTitle>
          <CardDescription className="text-xs sm:text-sm">Top 10 categories by product count</CardDescription>
        </CardHeader>
        <CardContent className="p-2 sm:p-6">
          <ChartContainer config={chartConfig} className="h-[250px] sm:h-[300px] w-full">
            <BarChart 
              data={categoryData} 
              layout="vertical" 
              margin={{ left: 0, right: 10, top: 5, bottom: 5 }}
            >
              <XAxis type="number" tick={{ fontSize: 10 }} />
              <YAxis 
                dataKey="name" 
                type="category" 
                width={70} 
                tick={{ fontSize: 9 }} 
                tickFormatter={(value) => value.length > 10 ? `${value.slice(0, 10)}...` : value}
              />
              <ChartTooltip content={<ChartTooltipContent />} />
              <Bar dataKey="value" radius={4}>
                {categoryData.map((_, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Bar>
            </BarChart>
          </ChartContainer>
        </CardContent>
      </Card>

      <Card className="overflow-hidden">
        <CardHeader className="pb-2">
          <CardTitle className="text-base sm:text-lg">Category Distribution</CardTitle>
          <CardDescription className="text-xs sm:text-sm">Pie chart of products per category</CardDescription>
        </CardHeader>
        <CardContent className="p-2 sm:p-6">
          <ChartContainer config={chartConfig} className="h-[250px] sm:h-[300px] w-full">
            <PieChart>
              <ChartTooltip content={<ChartTooltipContent />} />
              <Pie
                data={categoryData}
                dataKey="value"
                nameKey="name"
                cx="50%"
                cy="50%"
                outerRadius="70%"
                label={false}
              >
                {categoryData.map((_, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
            </PieChart>
          </ChartContainer>
          {/* Legend for mobile */}
          <div className="flex flex-wrap gap-2 mt-2 justify-center">
            {categoryData.slice(0, 5).map((item, index) => (
              <div key={item.name} className="flex items-center gap-1 text-xs">
                <div 
                  className="w-2 h-2 rounded-full" 
                  style={{ backgroundColor: COLORS[index % COLORS.length] }} 
                />
                <span className="text-muted-foreground truncate max-w-[60px] sm:max-w-[80px]">
                  {item.name}
                </span>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}


function formatDate(dateString: string) {
  return new Date(dateString).toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function getActionColor(action: string) {
  switch (action.toLowerCase()) {
    case "create":
      return "bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400";
    case "update":
      return "bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400";
    case "delete":
      return "bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400";
    default:
      return "bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-300";
  }
}

export function AuditLogsTable({ logs }: AuditLogsProps) {
  return (
    <Card>
      <CardHeader className="pb-2">
        <CardTitle className="text-base sm:text-lg">Recent Changes</CardTitle>
        <CardDescription className="text-xs sm:text-sm">Latest activity in the system</CardDescription>
      </CardHeader>
      <CardContent className="p-2 sm:p-6">
        {logs.length === 0 ? (
          <p className="text-sm text-muted-foreground text-center py-8">No recent changes</p>
        ) : (
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead className="text-xs">User</TableHead>
                  <TableHead className="text-xs">Action</TableHead>
                  <TableHead className="text-xs hidden sm:table-cell">Entity</TableHead>
                  <TableHead className="text-xs">Date</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {logs.map((log) => (
                  <TableRow key={log.id}>
                    <TableCell className="font-medium text-xs sm:text-sm py-2">{log.username}</TableCell>
                    <TableCell className="py-2">
                      <span className={`px-1.5 sm:px-2 py-0.5 sm:py-1 rounded-md text-xs ${getActionColor(log.action)}`}>
                        {log.action}
                      </span>
                    </TableCell>
                    <TableCell className="hidden sm:table-cell py-2">
                      <span className="text-muted-foreground text-xs">{log.entityType}:</span>{" "}
                      <span className="text-xs">{log.entityName || log.entityId.slice(0, 8)}</span>
                    </TableCell>
                    <TableCell className="text-muted-foreground text-xs py-2">{formatDate(log.createdAt)}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </CardContent>
    </Card>
  );
}

export function RecentProductsTable({ products }: RecentProductsProps) {
  return (
    <Card>
      <CardHeader className="pb-2">
        <CardTitle className="text-base sm:text-lg">Recent Products</CardTitle>
        <CardDescription className="text-xs sm:text-sm">Latest products added to inventory</CardDescription>
      </CardHeader>
      <CardContent className="p-2 sm:p-6">
        {products.length === 0 ? (
          <p className="text-sm text-muted-foreground text-center py-8">No recent products</p>
        ) : (
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead className="text-xs">Name</TableHead>
                  <TableHead className="text-xs hidden sm:table-cell">Category</TableHead>
                  <TableHead className="text-xs">Price</TableHead>
                  <TableHead className="text-xs">Stock</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {products.map((product) => (
                  <TableRow key={product.id}>
                    <TableCell className="font-medium text-xs sm:text-sm py-2 max-w-[120px] sm:max-w-none truncate">
                      {product.name}
                    </TableCell>
                    <TableCell className="text-muted-foreground text-xs hidden sm:table-cell py-2">
                      {product.categoryName}
                    </TableCell>
                    <TableCell className="text-xs sm:text-sm py-2">
                      {new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" }).format(product.price)}
                    </TableCell>
                    <TableCell className="py-2">
                      <span className={`text-xs sm:text-sm ${product.stockQuantity < 10 ? "text-destructive font-medium" : ""}`}>
                        {product.stockQuantity}
                      </span>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
