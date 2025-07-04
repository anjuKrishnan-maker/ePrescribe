export interface ReportMenuModel {
    ReportGroupList: ReportGroup[];
}

export interface ReportGroup {
    Name: string;
    ReportLinkList: ReportLink[];

    new(): ReportGroup;

    new(name: string): ReportGroup;
}

export interface ReportLink {
    Name: string;
    Description: string;
    Page: string;
    ShowDeluxeAd: boolean;
}