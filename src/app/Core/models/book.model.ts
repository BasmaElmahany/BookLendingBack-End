
export interface Book {
  id: number;
  title: string;
  description: string;
  author: string;
  publishedAt: string;
  isAvailable: boolean;
  catalogId: number;
  catalogName?: string; 
}


