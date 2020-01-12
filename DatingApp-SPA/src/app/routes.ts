import {Routes } from '@angular/router';
import { HomeComponent } from './Home/Home.component';
import { MembreListComponent } from './members/membre-list/membre-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MembreDetailComponent } from './members/membre-detail/membre-detail.component';
import { MemberDetailResolver } from './_resolvers/member.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MembreEditComponent } from './members/membre-edit/membre-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { ListsResolver } from './_resolvers/lists.resolver';
export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            {path: 'members', component: MembreListComponent, resolve: { users: MemberListResolver}},
            {path: 'members/:id', component: MembreDetailComponent, resolve: { user: MemberDetailResolver}},
            {path: 'member/edit', component: MembreEditComponent, resolve: {user: MemberEditResolver},
            canDeactivate: [PreventUnsavedChanges]},
            {path: 'messages', component: MessagesComponent},
            {path: 'lists', component: ListsComponent, resolve : {users: ListsResolver}},
        ]
    },
    {path: '**', redirectTo: 'home', pathMatch: 'full'},
];
