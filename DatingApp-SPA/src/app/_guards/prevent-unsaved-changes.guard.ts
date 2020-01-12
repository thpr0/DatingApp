import { Injectable } from '@angular/core';
import { CanDeactivate } from '../../../node_modules/@angular/router';
import { MembreEditComponent } from '../members/membre-edit/membre-edit.component';

@Injectable()
export class PreventUnsavedChanges implements CanDeactivate<MembreEditComponent> {
    canDeactivate(component: MembreEditComponent) {
        if (component.editForm.dirty) {
             return confirm('are you sure you want to continue?');
        }
        return true;
    }

}
