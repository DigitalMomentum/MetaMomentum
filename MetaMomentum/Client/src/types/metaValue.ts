export class MetaValue {

    // The title to be rendered on the frontend. This will be the fallback value if default.title is not set
    title: string = "";
    description: string = "";
    noIndex: boolean = false;
    shareTitle: string = "";
    shareDescription: string = "";
    shareImage: string | undefined = undefined;

    // The default section holds the values when a custom value is set by the user. Dont be misled by the name, this is not a fallback value or default value
    default: MetaCustomValues = {} as MetaCustomValues;

    // The share section holds the custom values entered by the user for for sharing
    share: MetaCustomShareValues = {} as MetaCustomShareValues;
}

export class MetaCustomValues {
    title: string = "";
    description: string = ""
}


export class MetaCustomShareValues {
    title: string = "";
    description: string = "";
    image: string | undefined = undefined;
}