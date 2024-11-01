export interface BitcoinItem {
	time: string;
	price: number;
}

export interface BitcoinTableItem {
	guid: string;
    note: string | null;
	price: number;
    time: string;
}