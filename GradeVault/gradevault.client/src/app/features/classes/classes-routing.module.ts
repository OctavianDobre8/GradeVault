import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClassViewComponent } from './class-view/class-view.component';

const routes: Routes = [{ path: '', component: ClassViewComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ClassesRoutingModule {}
