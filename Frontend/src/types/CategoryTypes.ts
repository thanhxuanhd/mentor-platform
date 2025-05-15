export interface Category {
    id: string;
    name: string;
    description?: string;
    courses: number;
    status: boolean;
}

export interface EditCategory {
    id: string;
    name: string;
    description?: string;
    status: boolean;
}

export interface CategoryFilter {
    pageSize: number;
    pageIndex: number;
    keyword: string;
}

export interface CategoryRequest {
    name?: string;
    description?: string;
    status: boolean;
}