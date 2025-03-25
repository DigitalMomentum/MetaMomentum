export interface MetaValue {
    title?: string;
    description?: string;
    noIndex?: boolean;
    shareTitle?: string;
    shareDescription?: string;
    shareImage?: string;
    default?:  {
        title: string,
        description: string
    };
    share?: {
        title?: string,
        description?: string,
        image?: string,
    }
}