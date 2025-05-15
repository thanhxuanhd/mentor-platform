export interface Category {
    id: string;
    name: string;
    description?: string;
    courses: number;
    status: boolean;
}

export interface EditCataegory {
    id: string;
    name: string;
    description?: string;
    status: 'Active' | 'Inactive';
}

export interface CategoryFilter {
    pageSize: number;
    pageIndex: number;
    keyword: string;
}
