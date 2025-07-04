export class RightBoxModel {
    pRightBoxHeaderText: string;
    pRightBoxImageURL: string;
    pRightBoxBodyText: string;
    Links: RightBoxLink[] = new Array;
}

export class RightBoxLink {
    Url: string;
    Text: string;
}