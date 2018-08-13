import oidc from 'oidc-client';

export class SecurityContext {
    private userManager: oidc.UserManager;

    constructor(config:ISecurityContextConfig) {
        this.userManager = SecurityContext.getUserManager(config);

        this.userManager.events.addSilentRenewError(error => {
            if (config.onSignInFailure)
                config.onSignInFailure();
        })
    }

    getAccessToken() {
        return this.userManager.getUser().then(user => {
            if (user == null || user.expired) {
                return this.userManager.signinSilent().then(x => {
                    return x.access_token
                });
            }
            return user.access_token;
        })
    }

    private static getUserManager(config:ISecurityContextConfig) {
        return new oidc.UserManager(config);
    }
}

export interface ISecurityContextConfig {
    authority: string;
    client_id: string;
    scope: string;
    response_type: string;
    silent_redirect_uri: string;
    onSignInFailure: () => void;
}