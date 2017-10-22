
import { IUser } from '../model';
import { default as GlobalConfig } from '../config';

export class CookieHelper {

    public static getCultureName(): string {

        let cookieValue = document.cookie.replace(/(?:(?:^|.*;\s*)\.AspNetCore\.Culture\s*\=\s*([^;]*).*$)|^.*$/, "$1");

        let cultureName: string = 'en';

        if (cookieValue) {
            let culture = decodeURIComponent(cookieValue).split('|');
            cultureName = culture[0].split('=')[1];
        }

        return cultureName;
    }

    public static getTimeZoneId(): string {

        let cookieValue = document.cookie.replace(/(?:(?:^|.*;\s*)\.AspNetCore\.Culture\s*\=\s*([^;]*).*$)|^.*$/, "$1");

        let timeZoneId: string = 'AUS Eastern Standard Time';

        if (cookieValue) {
            let culture = decodeURIComponent(cookieValue).split('|');
            
            if(culture.length > 2)
                timeZoneId = culture[2].split('=')[1];
        }

        return timeZoneId;
    }
}
