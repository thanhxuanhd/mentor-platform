export interface Category {
    id: string;
    name: string;
    description?: string;
    status: "Active" | "Inactive";
}